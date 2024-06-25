import psutil
import pyautogui

pyautogui.FAILSAFE = False
import pydirectinput

pydirectinput.FAILSAFE = False
import win32gui  # pip install pywin32
import win32process
import time
import mouse
import keyboard
from pynput.keyboard import Key, Controller, Listener

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


def face_to_waypoint(new_x, new_y, steps=12, sensitivity=12):
    current_x, current_y = pyautogui.position()
    x_diff = new_x - current_x
    y_diff = new_y - current_y
    step_x = x_diff / steps  # You can adjust the number of steps
    step_y = y_diff / steps

    # Perform a series of small mouse movements to reach the target location
    for _ in range(steps):
        pydirectinput.moveRel(xOffset=int(step_x * sensitivity), yOffset=int(step_y * sensitivity), relative=True,
                              disable_mouse_acceleration=True)
        # print("pydirection pos", pyautogui.position())
        time.sleep(0.1)


is_paused = False


def on_press(key):
    """
    pynput - keyboard listener function.

    :param key:
    """
    # nonlocal is_paused
    global is_paused
    if key == Key.f2:
        is_paused = not is_paused
        print(f'Loop is {"paused" if is_paused else "resumed"}')


def move_to_tracked_waypoint(waypoint_image, crosshair_x, crosshair_y, search_t=0.1, delay_t=5):
    wait_time = delay_t
    start_time = time.time()
    last_found_time = time.time()  # current time
    is_moving = False
    active = True
    is_alt = False
    previous_button_location = None

    keyboard = Controller()
    with Listener(on_press=on_press) as keyboard_listener:

        while True:
            try:
                if is_paused:
                    time.sleep(5)
                    continue
                search_time = search_t if active else delay_t
                elapsed_time = 1 if active else time.time() - start_time
                confidence = 0.4 if not active and elapsed_time < 8 else 0.5

                button_location = pyautogui.locateCenterOnScreen(waypoint_image, confidence=confidence, grayscale=True,
                                                                 region=(0, 0, 650, 459))
                if button_location is not None:
                    active = True

                    if not is_moving and not is_alt:
                        # mouse.move(crosshair_x, crosshair_y, absolute=True)
                        keyboard.press(Key.alt_l)
                        keyboard.release(Key.alt_l)
                        is_alt = True
                        time.sleep(3)

                    if previous_button_location is not None:
                        mouse.move(crosshair_x, crosshair_y, absolute=True)

                        xy_diff = max(abs(previous_button_location[0] - button_location[0]),
                                      abs(previous_button_location[1] - button_location[1]))

                        if xy_diff > 60:
                            keyboard.release('w')  # quick fix

                        if not is_moving and xy_diff <= 50:
                            keyboard.press('w')
                            is_moving = True

                        elif is_moving and xy_diff >= 60:
                            keyboard.release('w')
                            is_moving = False
                            time.sleep(0.2)

                        face_to_waypoint(button_location[0], button_location[1])

                    if elapsed_time >= 1:  # track image with  x seconds when not active
                        start_time = time.time()
                        elapsed_time = 0

                    last_found_time = time.time()  # Resets the last found time

                    previous_button_location = button_location  # Update previous_button_location
                    button_location = None

                    time.sleep(search_time)
                    continue

            except pyautogui.ImageNotFoundException:

                keyboard.release('w')  # Release the 'w' key immediately
                time.sleep(0.1)

                if is_moving:
                    keyboard.release('w')
                    is_moving = False
                    time.sleep(0.1)
                    if is_alt:
                        # time.sleep(0.4)
                        keyboard.press(Key.alt_l)
                        keyboard.release(Key.alt_l)
                        is_alt = False
                        # time.sleep(0.5)

                elif active:
                    if time.time() - last_found_time >= wait_time:
                        active = False
                    continue
                else:
                    # Stay in inactive state until the image is found
                    continue


def main():
    set_window_focus(get_pid())
    print(pyautogui.size())
    waypoint_image = "images/waypoint.jpg"
    move_to_tracked_waypoint(waypoint_image, 324, 250)


if __name__ == '__main__':
    main()

# See PyCharm help at https://www.jetbrains.com/help/pycharm/
