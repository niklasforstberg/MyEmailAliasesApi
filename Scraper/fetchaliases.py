import requests
from bs4 import BeautifulSoup
import os
from dotenv import load_dotenv
import re
import json

def login():
    # Load environment variables
    load_dotenv()
    
    # Create session to maintain cookies
    session = requests.Session()
    
    # Set up headers to look more like a browser
    session.headers.update({
        'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:134.0) Gecko/20100101 Firefox/134.0',
        'Accept': 'text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8',
        'Accept-Language': 'en-US,en;q=0.5',
        'Accept-Encoding': 'gzip, deflate, br',
        'Connection': 'keep-alive',
        'Upgrade-Insecure-Requests': '1'
    })
    
    print("\n=== Step 1: Getting initial login page ===")
    initial_url = "https://account.one.com/auth/realms/customer/protocol/openid-connect/auth"
    params = {
        'client_id': 'crm-appsrv',
        'redirect_uri': 'https://www.one.com/admin/',
        'response_type': 'code',
        'scope': 'openid',
        'login': 'true'
    }
    
    print(f"Initial URL: {initial_url}")
    print(f"Parameters: {json.dumps(params, indent=2)}")
    
    # Get the login page
    response = session.get(initial_url, params=params)
    print(f"Response status: {response.status_code}")
    print(f"Response URL: {response.url}")
    
    # Save initial response for inspection
    os.makedirs('tmp', exist_ok=True)
    with open('tmp/initial_response.html', 'w', encoding='utf-8') as f:
        f.write(response.text)
    
    # Extract execution ID from the form action URL
    soup = BeautifulSoup(response.text, 'html.parser')
    form = soup.find('form', id='kc-form-login')
    if not form:
        print("ERROR: Could not find login form!")
        return None
        
    # Extract execution ID from form action URL
    action_url = form.get('action', '')
    print(f"\nForm action URL: {action_url}")
    
    execution_match = re.search(r'execution=([^&]+)', action_url)
    if not execution_match:
        print("ERROR: Could not find execution ID!")
        return None
    
    execution_id = execution_match.group(1)
    print(f"Found execution ID: {execution_id}")
    
    # Get credentials
    username = os.getenv('ONE_COM_USERNAME')
    print(f"Username from .env: {username}")
    password = input("Enter your password: ")
    
    print("\n=== Step 2: Submitting login credentials ===")
    # First request - username and password
    login_data = {
        'username': username,
        'password': password,
        'credentialId': ''
    }
    print(f"Login data (password hidden): {json.dumps({**login_data, 'password': '*****'}, indent=2)}")
    print(f"Submitting to URL: {action_url}")
    
    # Update headers for the POST request
    session.headers.update({
        'Content-Type': 'application/x-www-form-urlencoded',
        'Origin': 'https://account.one.com',
        'Referer': response.url
    })
    print(f"Headers: {json.dumps(dict(session.headers), indent=2)}")
    
    # Send login request using the complete action URL
    response = session.post(action_url, data=login_data)
    print(f"Response status: {response.status_code}")
    print(f"Response URL: {response.url}")
    
    # Save response to tmp directory
    with open('tmp/login_response.html', 'w', encoding='utf-8') as f:
        f.write(response.text)
    
    # Check for error messages in the response
    soup = BeautifulSoup(response.text, 'html.parser')
    error_msg = soup.find(class_='kc-feedback-text')
    if error_msg:
        print(f"\nERROR from server: {error_msg.text.strip()}")
    
    # Check if login was successful and 2FA is required
    if response.status_code == 200:
        soup = BeautifulSoup(response.text, 'html.parser')
        otp_form = soup.find('form', id='kc-otp-login-form')
        
        if otp_form:
            print("\n=== Step 3: 2FA Required ===")
            totp_code = input("Enter the 6-digit code from your authenticator app: ")
            
            # Get the action URL from the form
            totp_submit_url = otp_form.get('action')
            print(f"2FA form action URL: {totp_submit_url}")
            
            totp_data = dict(
                otp=totp_code,
                login='Log In'
            )
            
            # Update headers specifically for 2FA submission
            session.headers.update({
                'Content-Type': 'application/x-www-form-urlencoded',
                'Origin': 'https://account.one.com',
                'Referer': response.url
            })
            
            print(f"2FA data: {json.dumps(totp_data, indent=2)}")
            print("Request Headers:")
            print(json.dumps(dict(session.headers), indent=2))
            print(f"Cookies: {session.cookies.get_dict()}")
            
            # Try without allow_redirects first to see the immediate response
            totp_response = session.post(totp_submit_url, data=totp_data, allow_redirects=False)
            print(f"\n2FA Response:")
            print(f"Status Code: {totp_response.status_code}")
            print(f"URL: {totp_response.url}")
            print(f"Response Headers:")
            print(json.dumps(dict(totp_response.headers), indent=2))
            
            if totp_response.status_code == 302:
                # Step 1: Go directly to mail overview
                mail_url = "https://www.one.com/admin/mail/overview.do"
                session.headers.update({
                    'Accept': 'text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8',
                    'Upgrade-Insecure-Requests': '1'
                })
                
                mail_response = session.get(mail_url)
                print(f"\nMail Overview Response Status: {mail_response.status_code}")
                
                # Step 2: Make the API request with proper headers
                domain = os.getenv('ONE_COM_DOMAIN')  # Add this to your .env file
                api_url = f"https://www.one.com/admin/api/domains/{domain}/mail/overview"
                
                session.headers.update({
                    'Accept': 'application/json, text/plain, */*',
                    'X-Requested-With': 'XMLHttpRequest',
                    'Referer': 'https://www.one.com/admin/mail/overview.do',
                    'Content-Type': 'application/json'
                })
                
                api_response = session.get(api_url)
                print(f"\nAPI Response Status: {api_response.status_code}")
                print(f"API Response: {api_response.text}")
                
                with open('tmp/mail_data.json', 'w', encoding='utf-8') as f:
                    f.write(api_response.text)
                
                if api_response.status_code == 200:
                    print("\nSuccessfully fetched mail data!")
                    return session
        else:
            print("\nNo 2FA form found - checking if login succeeded...")
            if 'admin' in response.url:
                print("Login successful!")
                return session
            else:
                print("Login failed!")
    else:
        print(f"\nLogin failed with status code: {response.status_code}")
    
    return None

def process_mail_data():
    """Process the mail data from the cached JSON file"""
    print("\nProcessing cached mail data...")
    with open('tmp/mail_data.json', 'r') as f:
        data = json.load(f)
    
    # TODO: Process data and store in database
    # For now, just show how many aliases we found
    aliases = [addr for addr in data['result']['addresses'] 
              if addr['type'] == 'ALIAS']
    print(f"Found {len(aliases)} aliases")
    return True

def main():
    try:
        # Check if we have cached data
        if os.path.exists('tmp/mail_data.json'):
            process_mail_data()
        else:
            print("No cached data found, logging in...")
            session = login()
            if session:
                print("Login successful!")
            else:
                print("Login failed!")
                
    except Exception as e:
        print(f"\nAn error occurred: {str(e)}")

if __name__ == "__main__":
    main()
