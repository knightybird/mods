import time
import os
import pyautogui


def send_notification():
    shared_folder = "Z:/main client"
    notification_file = os.path.join(shared_folder, "notification.txt")

    with open(notification_file, "w") as file:
        file.write("Notification triggered at: " + time.strftime("%Y-%m-%d %H:%M:%S"))


def find_revive_button(image):
    while True:
        try:
            button_location = pyautogui.locateCenterOnScreen(image, confidence=0.5, grayscale=True)
            if button_location is not None:
                send_notification()
                time.sleep(10)
                continue

        except pyautogui.ImageNotFoundException:
            time.sleep(10)
            continue


if __name__ == '__main__':
    revive_button_image = "images/revive.PNG"
    find_revive_button(revive_button_image)
