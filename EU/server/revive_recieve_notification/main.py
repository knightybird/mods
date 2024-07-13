import time
import os
from dotenv import load_dotenv

import pyautogui

load_dotenv()  # Load variables from .env file

# Host PC settings
shared_folder = os.getenv("SHARED_FOLDER")
notification_file = os.path.join(shared_folder, "notification.txt")

while True:
    if os.path.exists(notification_file):
        # Notification file found, send a desktop notification
        pyautogui.alert(text="Notification from VM!", title="VM Alert", button="OK")
        # Remove the notification file after sending the alert
        os.remove(notification_file)

    # Wait for a specified interval before checking again
    time.sleep(10)  # Check every minute
