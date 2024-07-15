import os
import time
import random
from dotenv import load_dotenv

import werkzeug

werkzeug.cached_property = werkzeug.utils.cached_property  # fix, must be above robobrowser

from robobrowser import RoboBrowser

load_dotenv(dotenv_path=".env")


def login_and_navigate():
    """
    Perform login and navigation actions.
    """
    print("login_and_navigate")
    browser = RoboBrowser(history=True)
    browser.open('https://www.lootius.io/')
    form = browser.get_form(action='/User/Login')
    form['user'].value = os.getenv("USER")
    form['pass'].value = os.getenv("PASS")
    browser.submit_form(form)
    browser.open('https://www.lootius.io/CrateHunt')
    # if browser = "url=https://www.lootius.io/NotLoggedIn":
    #     print("Login successful and navigated to CrateHunt page.")


def countdown(seconds):
    """
    Display a countdown timer on the command line without creating new lines.
    """
    print("Countdown started!")
    while seconds:
        mins, secs = divmod(seconds, 60)
        hours, mins = divmod(mins, 60)
        timer = '{:02d}:{:02d}:{:02d}'.format(hours, mins, secs)
        print(f'\r{timer}', end='')
        time.sleep(1)
        seconds -= 1
    print('\nCountdown finished!')


def hourly_task():
    """
    Execute the hourly task with a random delay.
    """
    login_and_navigate()

    print("start hourly task")
    delay = random.randint(3660, 3672)
    print(f"Waiting for {countdown(delay)} seconds before executing the task...")


if __name__ == '__main__':
    print("running")

    while True:
        hourly_task()
