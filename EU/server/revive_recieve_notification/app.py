from flask import Flask, jsonify, Response

# for localhost testing
from flask_cors import CORS

app = Flask(__name__)
CORS(app)  # Enable CORS - localhost


@app.route("/", methods=["GET"])
def main():
    pass


if __name__ == "__main__":
    app.run(host="0.0.0.0", port=8000, debug=True)
