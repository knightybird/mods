import pyautogui


revive_button = "images/revive.PNG"
button_location = pyautogui.locateCenterOnScreen(revive_button, confidence=0.5, grayscale=True,
                                                 region=(0, 0, 650, 459))
if button_location is not None:
    pixel_color_name = "Red"
else:
    pixel_color_name = "Black"
