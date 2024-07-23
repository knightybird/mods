import time

import keyboard

from pynput.keyboard import Key, Controller, Listener

is_paused_F = True
user_pressed = True

class MyException(Exception):
    pass


def on_press_f(key):
    """
    pynput - keyboard listener function.

    :param key:
    """
    # nonlocal is_paused
    global is_paused_F
    global user_pressed
    try:
        if key.char == 'f':
            is_paused_F = not is_paused_F
            print(f'Loop is {"paused" if is_paused_F else "resumed"}')

    except AttributeError as e:
        pass


def auto_hunt():
    keyboard_controller = Controller()
    with Listener(on_press=on_press_f) as keyboard_listener:
        while True:
            try:
                if is_paused_F:
                    time.sleep(1)
                    continue
                else:
                    global user_pressed
                    user_pressed = False
                    while not is_paused_F and not user_pressed:
                        time.sleep(0.25)
                        with keyboard_controller.pressed(Key.shift):
                            keyboard.press('f')
                            keyboard.release('f')
                    user_pressed = True
            except MyException as e:
                print('{0} was pressed'.format(e.args[0]))


def main():
    auto_hunt()


if __name__ == '__main__':
    main()
# See PyCharm help at https://www.jetbrains.com/help/pycharm/
