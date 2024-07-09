import time
import os
import pyautogui

# Host PC settings
shared_folder = "C:\\_Home\\Nick\\Entertainment\\main client"
notification_file = os.path.join(shared_folder, "notification.txt")

while True:
    if os.path.exists(notification_file):
        # Notification file found, send a desktop notification
        pyautogui.alert(text="Notification from VM!", title="VM Alert", button="OK")
        # Remove the notification file after sending the alert
        os.remove(notification_file)

    # Wait for a specified interval before checking again
    time.sleep(10)  # Check every minute
