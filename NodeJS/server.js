var websocket = require('ws');

var websocketServer = new websocket.Server({port:36500},()=> {
    console.log("DOme Test Server is running");
});

var wsList = [];

websocketServer.on("connection" , (ws, rq)=>{

    console.log('Connected.');
    
    wsList.push(ws);

    ws.on("message" , (data)=>{

        console.log("send from cilent : "+data);
        Boardcast(data);
    });

    ws.on("close", ()=>{

        wsList = ArrayRemove(wsList, ws);
        console.log("disconnected.");
    });
});

function ArrayRemove(arr, value)
{
    return arr.filter((element)=>{
        return element != value;
    })
}

function Boardcast(data)
{
    for(var i=  0; i < wsList.length; i++)
    {
        wsList[i].send(data);
    }
}