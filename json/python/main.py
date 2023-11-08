import json
import bcrypt
import os

# Get the directory of the current script
current_script_dir = os.path.dirname(os.path.abspath(__file__))

# Path to the 'users.json' file within the 'python' folder
users_file_path = os.path.join(current_script_dir, 'users.json')

def signup(username, email, password):
    # Hash the password
    hashed_password = bcrypt.hashpw(password.encode('utf-8'), bcrypt.gensalt())

    # Initialize users dictionary
    users = {}

    # Check if the 'users.json' file exists and has content
    if os.path.isfile(users_file_path) and os.path.getsize(users_file_path) > 0:
        # Load existing users from the JSON file
        with open(users_file_path, 'r') as file:
            users = json.load(file)
    else:
        # Create the 'users.json' file if it doesn't exist or is empty
        with open(users_file_path, 'w') as file:
            file.write('{}')  # Write an empty JSON object

    # Check if the username or email already exists
    if username in users or email in [user['email'] for user in users.values()]:
        print("Username or email already exists")
        return

    # Add the new user to the list
    users[username] = {'email': email, 'password': hashed_password.decode('utf-8')}

    # Write the updated users dictionary back to the JSON file
    with open(users_file_path, 'w') as file:
        json.dump(users, file, indent=4)

def login(username_or_email, password):
    # Check if the 'users.json' file exists and has content
    if not os.path.isfile(users_file_path) or os.path.getsize(users_file_path) == 0:
        print("No users found")
        return False

    # Load existing users from the JSON file
    with open(users_file_path, 'r') as file:
        users = json.load(file)

    # Determine if the user is logging in with username or email
    user = None
    if '@' in username_or_email:  # User is attempting to log in with an email
        for u, data in users.items():
            if data['email'] == username_or_email:
                user = u
                break
    else:
        user = users.get(username_or_email)

    # If user is found by username or email, verify the password
    if user and bcrypt.checkpw(password.encode('utf-8'), users[user]['password'].encode('utf-8')):
        return True

    return False

def is_valid_email(email):
    parts = email.split('@')
    if len(parts) != 2 or not parts[1].count('.') >= 1:
        return False
    return True

def main():
  while True:
      # Ask the user to choose between signup and login
      choice = input("Do you want to signup or login? ")

      if choice.lower() == 'signup':
          # Ask the user to enter a username, a valid email, and a password twice
          username = input("Enter a username: ")
          # Check if the 'users.json' file exists and has content
          if os.path.isfile(users_file_path) and os.path.getsize(users_file_path) > 0:
              # Load existing users from the JSON file
              with open(users_file_path, 'r') as file:
                 users = json.load(file)
                 # Check if the username already exists
                 if username in users:
                     print("Username already exists, please try again")
                     continue
          email = input("Enter a valid email: ")
          # Check if the email already exists
          if email in [user['email'] for user in users.values()]:
              print("Email already exists, please try again")
              continue
          if is_valid_email(email):
              password = input("Enter a password: ")
              while True:
                 password2 = input("Enter the password again: ")
                 if password == password2:
                     break
                 else:
                     print("Passwords do not match, please try again")
              signup(username, email, password)
              print("Signup successful")
              break
          else:
              print("Invalid email address, please try again")
              continue
      elif choice.lower() == 'login':
          # Ask the user to enter their username or email and the correct password
          username = input("Enter your username or email: ")
          password = input("Enter your password: ")
          if login(username, password):
              print("Login successful")
              break
          else:
              print("Invalid username or password")
              continue
      else:
          print("Invalid choice, please try again")
          continue

if __name__ == "__main__":
  main()