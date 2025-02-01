import requests
import json
import time
import os
import getpass
from dotenv import load_dotenv

class OneDotComClient:
    def __init__(self):
        self.session = requests.Session()
        self.base_url = "https://account.one.com"
        
        # Set common headers
        self.session.headers.update({
            'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:134.0) Gecko/20100101 Firefox/134.0',
            'Accept': 'text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8',
            'Accept-Language': 'en-US,en;q=0.5'
        })

    def login(self, username, password, totp_code):
        """
        Perform login to one.com including 2FA
        """
        # Step 1: Initial login with username and password
        login_url = f"{self.base_url}/auth/realms/customer/login-actions/authenticate"
        
        login_data = {
            'username': username,
            'password': password,
            'credentialId': ''
        }

        response = self.session.post(
            login_url,
            data=login_data,
            allow_redirects=True
        )

        # Check if we got the 2FA page
        if "Google Authenticator" in response.text:
            totp_data = {
                'otp': totp_code
            }
            
            response = self.session.post(
                response.url,  # Use the current URL for 2FA submission
                data=totp_data,
                allow_redirects=True
            )

        return response.ok

    def get_aliases(self):
        """
        Fetch email aliases after successful login
        """
        # You'll need to implement the logic to navigate to and parse the aliases page
        # This is a placeholder for the actual implementation
        aliases_url = "https://www.one.com/admin/email-aliases"  # You'll need to verify the correct URL
        
        response = self.session.get(aliases_url)
        
        # Parse the response and extract aliases
        # This will depend on the actual structure of the page
        
        return []  # Return the list of aliases

def main():
    # Load environment variables
    load_dotenv()
    
    # Create client instance
    client = OneDotComClient()
    
    # Get credentials
    username = os.getenv('USERNAME')
    password = getpass.getpass("Enter your password: ")
    totp_code = input("Enter the 6-digit code from your authenticator app: ")
    
    try:
        # Attempt login
        if client.login(username, password, totp_code):
            print("Login successful!")
            
            # Fetch aliases
            aliases = client.get_aliases()
            print("Email aliases:", aliases)
        else:
            print("Login failed!")
            
    except Exception as e:
        print(f"An error occurred: {str(e)}")

if __name__ == "__main__":
    main()
