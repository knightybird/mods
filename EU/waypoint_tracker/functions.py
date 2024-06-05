import pyautogui

pyautogui.FAILSAFE = False
import pydirectinput

pydirectinput.FAILSAFE = False
import pyperclip
import time
from pynput.keyboard import Key, Controller


def revive():
    # If image is not found After 3 seconds wait time...
    time.sleep(1)
    t_end = time.time() + 3
    while time.time() < t_end:
        try:
            x, y = pyautogui.locateCenterOnScreen("images/revive-button.jpg", confidence=0.8, grayscale=True)
            time.sleep(1)
            pydirectinput.mouseDown(x, y)
            pydirectinput.mouseUp(x, y)
            break

        except:
            pass


def move():
    pyautogui.mouseDown(button='right')
    time.sleep(1)
    pyautogui.mouseUp(button='right')


def toggle_hud():
    time.sleep(1)
    keyboard = Controller()
    keyboard.press(Key.f1)
    keyboard.release(Key.f1)
    time.sleep(1)


def get_waypoint(waypoints_txt, index):
    with open('wp_sparta.txt', 'r') as file:
        # for i, line in enumerate(file):
        #     print(f"Line {i + 1}: {line.strip()}")
        #     text = line.strip()
        #     pyperclip.copy(text)

        lines = file.readlines()
        print(len(lines))
        print(lines[2].strip())


def paste():
    pyautogui.typewrite(pyperclip.paste(), interval=0.001)


def paste_waypoint():
    try:
        pyautogui.moveTo(100, 470)
        pyautogui.click()
    except:
        pass
    # Move the mouse to the location of the text field

    # Simulate a left-click to select the text field
    keyboard2 = Controller()
    keyboard2.tap(Key.enter)
    time.sleep(0.5)

    paste()
    # pyautogui.PAUSE = 1.0
    # pyautogui.typewrite(pyperclip.paste())
    # pyautogui.PAUSE = 1.0

    keyboard2.tap(Key.enter)


def track_waypoint(waypoint_image):
    button_location = None
    while True:
        try:
            button_location = pyautogui.locateCenterOnScreen(waypoint_image, confidence=0.5, grayscale=True)
            print('Found it!')
            print(pyautogui.size())
            print(button_location[0], button_location[1])
            time.sleep(1)
            break
        except Exception as e:
            # print(f"An error occurred: {e}")
            pass
    if button_location is not None:
        face_to_waypoint(324, 250, button_location[0], button_location[1])
    else:
        print("Button not found.")


def move_to_waypoint(waypoint_image, delay=5):
    keyboard = Controller()
    keyboard.press(Key.alt_l)
    keyboard.release(Key.alt_l)

    wait_time = delay
    search_time = 0.1
    last_found_time = time.time()  # current time
    key_pressed = False
    active_state = True
    while True:
        try:
            waypoint_position = pyautogui.locateCenterOnScreen(waypoint_image, confidence=0.5, grayscale=True)
            time.sleep(search_time)

            if waypoint_position is not None:
                if not key_pressed:
                    keyboard.press('w')
                    key_pressed = True
                last_found_time = time.time()  # Updates the last found time

            elif key_pressed:
                keyboard.release('w')
                key_pressed = False

            else:
                # If image not found and button released, then
                # Switch to inactive state if image not found since x seconds
                if time.time() - last_found_time >= wait_time:
                    active_state = False

                continue

        except pyautogui.ImageNotFoundException:

            keyboard.release('w')  # Release the 'w' key immediately
            key_pressed = False

            if active_state:
                continue

            else:
                # Stay in inactive state until the image is found
                keyboard.release('w')
                search_time = wait_time
                continue

    keyboard.press(Key.alt_l)
    keyboard.release(Key.alt_l)
