import time
import os

import mouse
import pyautogui
from pynput.keyboard import Key, Controller, Listener

shared_folder = "Z:/SHARED programs/commands"
follow_file = os.path.join(shared_folder, "follow.txt")


def group_follow():
    # mouse move  to 65 230  and left click
    # wait 850ms
    # move 65 155 and double right click
    # wait 850ms
    # move 100 155 to 75 150
    # click 75 165
    keyboard = Controller()

    crosshair_x = 65
    crosshair_y = 230
    mouse.move(crosshair_x, crosshair_y, absolute=True)
    time.sleep(0.85)
    mouse.move(65, 155, absolute=True)
    time.sleep(0.85)
    pyautogui.click(clicks=2, interval=0.3)

    keyboard.press('f')
    time.sleep(2)
    keyboard.release('f')
    time.sleep(2.85)
    mouse.move(85, 160, absolute=True)
    time.sleep(0.85)
    mouse.click()
    time.sleep(0.3)
    mouse.click()
    time.sleep(0.85)

    mouse.move(crosshair_x, crosshair_y, absolute=True)
    time.sleep(0.85)
    mouse.click()
    time.sleep(0.3)
    mouse.click()


def main():
    print(pyautogui.size())

    while True:
        if os.path.exists(follow_file):
            group_follow()
            time.sleep(10)
        time.sleep(1)
        continue


if __name__ == '__main__':
    main()
