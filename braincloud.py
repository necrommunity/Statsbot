import httpx
import json as jsonlib

SOCKET_PATH = "/tmp/express_ipc.sock"

# Direct HTTP requests through the Unix socket transport
transport = httpx.HTTPTransport(uds=SOCKET_PATH)


# type: search, username: str
# type: steamid, uuid: str
# type: getlb, lbid: str, index: int
def query(**kwargs):
    url = "http://localhost/"
    if not kwargs['type']:
        task = 'query'
        payload = {
        "queryId": "q_1001",
        "payload": "SELECT * FROM local_cache"}
    else:
        task = kwargs['type']
        payload = kwargs

    with httpx.Client(transport=transport) as client:
        response = client.post(url+task, json=payload)
        response.raise_for_status()
        return response.json()



def fetch_lb(lbid, index):
    res = query(type='getlb', lbid=lbid, index=index)
    if res:
        return res['leaderboard']
    
def search_users(name):
    res = query(type='search', username=name)
    if len(res) == 0:
        return None
    else:
        return res[0]
   
def get_steamid(uuid):
    res = query(type='steamid', uuid=uuid)
    if res:
        return res['externalId']
    

if __name__ == "__main__":
   # execute_query()
    taskQueue = [
	{ 'type': "search", 'username': "naymin" },
    { 'type': 'steamid', 'uuid': "513ef17d-ec65-49d5-bc40-65aff3db2420"},
	{ 'type': "steamid", 'uuid': "f2344da2-2ac8-4e75-8dae-b3d36664ac1f"},
	{ 'type': "getlb", 'lbid': "DLC_HARDCORE_Aria_PROD", 'index': 10},
    { 'type': "getlb", 'lbid': "DLC_SPEEDRUN_Aria_PROD", 'index': 10}]
    
    res = query(**taskQueue[-1])
    print(fetch_lb(taskQueue[-1], res))
    
    res2 = query(**taskQueue[0])
    
    print(query(**taskQueue[1]))
    
    for t in taskQueue[-2:-1]:
        res = query(**t)
        # parselb(t, res)