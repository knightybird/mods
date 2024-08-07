import os
import re
from collections import defaultdict

# Set the directory path containing the log files
log_dir = './'

# Create a dictionary to store the latest message for each user
user_messages = defaultdict(dict)

# Iterate through each log file in the directory
for filename in os.listdir(log_dir):
    if filename.endswith('.log'):
        filepath = os.path.join(log_dir, filename)
        with open(filepath, 'r', encoding='utf-8') as file:
            # Iterate through each line in the log file
            for line in file:
                # Extract the timestamp, username, and message using regular expressions
                timestamp, username, message = re.match(r'(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}) \[(.*?)\] (.*)', line).groups()
                # Remove leading and trailing whitespace from the message
                message = message.strip()
                # Check if the message is already stored for the user
                if username in user_messages and message in user_messages[username]:
                    # If the message is already stored, compare the timestamps
                    existing_timestamp = user_messages[username][message]
                    if timestamp > existing_timestamp:
                        # If the current message is newer, update the stored message
                        user_messages[username][message] = timestamp
                else:
                    # If the message is not stored, add it to the dictionary
                    user_messages[username][message] = timestamp

# Create a set to store the lines to be deleted
lines_to_delete = set()

# Iterate through each log file again to mark lines for deletion
for filename in os.listdir(log_dir):
    if filename.endswith('.log'):
        filepath = os.path.join(log_dir, filename)
        with open(filepath, 'r', encoding='utf-8') as file:
            for line in file:
                timestamp, username, message = re.match(r'(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}) \[(.*?)\] (.*)', line).groups()
                message = message.strip()
                if username in user_messages and message in user_messages[username]:
                    existing_timestamp = user_messages[username][message]
                    if timestamp < existing_timestamp:
                        # Mark the line for deletion if it's an older duplicate
                        lines_to_delete.add(line)

# Iterate through each log file again to delete marked lines
for filename in os.listdir(log_dir):
    if filename.endswith('.log'):
        filepath = os.path.join(log_dir, filename)
        with open(filepath, 'r', encoding='utf-8') as file:
            lines = file.readlines()
        with open(filepath, 'w', encoding='utf-8') as file:
            for line in lines:
                if line not in lines_to_delete:
                    file.write(line)