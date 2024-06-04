folder /waypoint_tracker
Build with pyinstaller
pyinstaller -F AG-sparta.py
pyinstaller -F main.py


folder /waypoint_tracker/waypoints2exe/setup.py
Build with cxfreeze
python setup.py bdist_msi
