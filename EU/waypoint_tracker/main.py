import time

import psutil
import pyautogui

pyautogui.FAILSAFE = False
import pydirectinput

pydirectinput.FAILSAFE = False
import pyperclip
import win32api
import win32gui  # pip install pywin32
import win32process
import win32con
import time
import mouse
import keyboard
from pynput.keyboard import Key, Controller

from pywinauto import Application


def get_process_id(name):
    """
    Returns the process ID of the process with the specified name.

    :param name: The name of the process.
    :return: The process ID of the process.
    """
    for proc in psutil.process_iter():
        if proc.name() == name:
            return proc.pid

    return None


def get_pid():
    pid = get_process_id("Entropia.exe")
    if pid is not None:
        app = Application().connect(process=pid)
        app.top_window().set_focus()


def set_window_focus(pid):
    """
    Brings the window with the specified process ID to the foreground.

    :param pid: The process ID of the window.
    """

    def enum_windows_callback(hwnd, lparam):
        """
        Callback function for EnumWindows.

        :param hwnd: The handle of the window.
        :param lparam: The parameter passed to EnumWindows.
        """
        if win32process.GetWindowThreadProcessId(
                hwnd) == win32process.GetCurrentProcessId() and win32gui.IsWindowVisible(hwnd):
            win32gui.SetForegroundWindow(hwnd)
            return False
        return True

    process_id = (pid, 0)
    win32gui.EnumWindows(enum_windows_callback, None)


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


def face_to_waypoint(x, y, new_x, new_y, steps=12):
    print("crosshair pos", x, y)
    print("target position: ", new_x, new_y)

    current_x, current_y = pydirectinput.position()
    crosshair_x = x
    crosshair_y = y
    # Calculate the difference between the current and target positions
    dx, dy = crosshair_x - current_x, crosshair_y - current_y
    for _ in range(steps):
        pydirectinput.move(int(dx / steps), int(dy / steps))
        print("pyautogui pos", pyautogui.position())

        time.sleep(0.01)

    # mouse.move(x, y, absolute=True)


    current_x, current_y = pyautogui.position()
    print("current pydirection pos", current_x, current_y)

    x_diff = new_x - current_x
    y_diff = new_y - current_y
    print("dx, dy: ", x_diff, y_diff)

    step_x = x_diff / steps  # You can adjust the number of steps
    step_y = y_diff / steps
    print("step x, y: ", int(step_x), int(step_y))
    # Perform a series of small mouse movements to reach the target location

    keyboard = Controller()
    keyboard.press(Key.alt_l)
    keyboard.release(Key.alt_l)
    time.sleep(3)
    # Perform a series of small mouse movements to reach the target location
    for _ in range(steps):
        # current_x += step_x
        # current_y += step_y
        pydirectinput.moveRel(xOffset=int(step_x*12), yOffset=int(step_y*12), relative=True, disable_mouse_acceleration=True)
        # pydirectinput.moveTo(int(current_x / 7), int(current_y / 7))
        print("pydirection pos", pyautogui.position())
        time.sleep(0.1)

    # print("new position: ", pydirectinput.position())
    # print("new position: ", mouse.get_position())
    keyboard.press(Key.alt_l)
    keyboard.release(Key.alt_l)


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


def move_to_tracked_waypoint(waypoint_image, delay=5):
    # keyboard = Controller()
    # keyboard.press(Key.alt_l)
    # keyboard.release(Key.alt_l)

    wait_time = delay
    search_time = 0.1
    last_found_time = time.time()  # current time
    is_moving = False
    active_state = True

    while True:
        try:
            button_location = pyautogui.locateCenterOnScreen(waypoint_image, confidence=0.5, grayscale=True)
            time.sleep(search_time)

            if button_location is not None:

                if not is_moving:
                    face_to_waypoint(324, 250, button_location[0], button_location[1])

                    time.sleep(1)  # Wait x seconds before moving
                    is_moving = True
                    keyboard.press('w')
                last_found_time = time.time()  # Updates the last found time

            else:
                if is_moving:
                    is_moving = False
                    keyboard.release('w')

                else:
                    # If image not found and button released, then
                    # Switch to inactive state if image not found since x seconds
                    if time.time() - last_found_time >= wait_time:
                        active_state = False

                    continue

        except pyautogui.ImageNotFoundException:

            if is_moving:
                keyboard.release('w')  # Release the 'w' key immediately
                is_moving = False

            if active_state:
                continue

            else:
                # Stay in inactive state until the image is found
                keyboard.release('w')
                search_time = wait_time
                continue

    # keyboard.press(Key.alt_l)
    # keyboard.release(Key.alt_l)


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


def main():
    set_window_focus(get_pid())
    time.sleep(1)
    waypoint_image = "images/waypoint.jpg"
    track_waypoint(waypoint_image)
    move_to_waypoint(waypoint_image)


if __name__ == '__main__':
    main()

# See PyCharm help at https://www.jetbrains.com/help/pycharm/
