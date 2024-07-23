import time

import pyautogui
from pynput.keyboard import Key, Controller, Listener
import os
from dotenv import load_dotenv
pyautogui.FAILSAFE = False

load_dotenv()  # Load variables from .env file

# Host PC settings
shared_folder = os.getenv("SHARED_FOLDER")
follow_file = os.path.join(shared_folder, "follow.txt")

is_paused = False


def on_press_follow_lead(key):
    """
    pynput - keyboard listener function.

    :param key:
    """
    # nonlocal is_paused
    global is_paused
    if key == Key.f2:
        is_paused = not is_paused

        print(f'file is {"removed" if is_paused else "created"}')


def follow_lead():
    keyboard = Controller()
    with Listener(on_press=on_press_follow_lead) as keyboard_listener:

        while True:
            try:
                if is_paused and os.path.exists(follow_file):
                    os.remove(follow_file)
                    time.sleep(1)
                elif not is_paused and not os.path.exists(follow_file):
                    with open(follow_file, "w") as file:
                        file.write("")
                        time.sleep(1)
                time.sleep(1)

            except Exception as e:
                pass


def main():
    print(pyautogui.size())
    follow_lead()


if __name__ == '__main__':
    main()
