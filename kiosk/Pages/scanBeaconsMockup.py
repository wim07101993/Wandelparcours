import random
import sys
import flask
import json
list = []
list.append(dict(id="1", rssi=random.randint(0, 50)))
list.append(dict(id="2", rssi=random.randint(0, 50)))
list.append(dict(id="3", rssi=random.randint(0, 50)))
list.append(dict(id="4", rssi=random.randint(0, 50)))
print(json.dumps(list))
sys.stdout.flush()