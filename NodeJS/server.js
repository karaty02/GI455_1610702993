var websocket = require('ws');

var websocketServer = new websocket.Server({port:36500},()=> {
    console.log("DOme Test Server is running");
});

var wsList = [];
var roomList = [];

websocketServer.on("connection" , (ws, rq)=>{
    {
        //LobbyZone
        ws.on("message" , (data)=>{
            
            console.log(data);

            var toJson = JSON.parse(data);

            //console.log(toJson.eventName);

            if(toJson.eventName == "CreateRoom")//CreateRoom
            {
                console.log("client request CreateRoom ["+toJson.data+"]");
                var isFoundRoom = false;
                for (let i = 0; i < roomList.length; i++) 
                {
                    if (roomList[i].roomName == toJson.data)
                    {
                        isFoundRoom = true;
                        break;
                    }
                }

                if(isFoundRoom)
                {
                    //Callclack to client : roomName is exist.
                    console.log("Create room : room is found");

                    var resultData = {
                        eventName: toJson.eventName,
                        data: "fail"
                    }

                    var toJsonStr = JSON.stringify(resultData)

                    ws.send(toJsonStr);
                }
                else
                {
                    //Create room here.
                    console.log("Create room : room is not found");

                    var newRoom = 
                    {
                        roomName: toJson.data,
                        wsList: []
                    }

                    newRoom.wsList.push(ws);

                    roomList.push(newRoom);

                    var resultData = {
                        eventName: toJson.eventName,
                        data: "Success"
                    }

                    var toJsonStr = JSON.stringify(resultData)

                    ws.send(toJsonStr);
                }
            }
            else if(toJson.eventName == "JoinRoom")

            {
                console.log("client request JoinRoom ["+toJson.data+"]");

                var isFoundRoom = false;
            
                for (let i = 0; i < roomList.length; i++) 
                {
                    if (roomList[i].roomName == toJson.data)
                    {
                        roomList[i].wsList.push(ws);
                        isFoundRoom = true;
                        break;
                    }
                
                }

                if(isFoundRoom)
                {
                    //Callclack to client : roomName is exist.
                    console.log("JoinRoom : room is found");

                    var resultData = {
                        eventName: toJson.eventName,
                        data: "Success"
                    }

                    var toJsonStr = JSON.stringify(resultData)

                    ws.send(toJsonStr);
                }
                else
                {
                    //Create room here.
                    console.log("JoinRoom : room is not found");


                    var resultData = {
                        eventName: toJson.eventName,
                        data: "fail"
                    }

                    var toJsonStr = JSON.stringify(resultData)

                    ws.send(toJsonStr);
                }
            }
            else if(toJson.eventName == "LeaveRoom")
            {
                    var isLeaveSuccess = false;
                for(var i = 0; i < roomList.length; i++)
                {
                    for(var j = 0; j < roomList[i].wsList.length; j++)
                    {   
                        if(ws == roomList[i].wsList[j])
                        {
                            roomList[i].wsList.splice(j, 1);

                            if(roomList[i].wsList.length <= 0)
                            {
                                roomList.splice(i, 1);
                            }
                            isLeaveSuccess = true;
                            break;
                        }
                    }
                }

                if(isLeaveSuccess)
                {
                    var resultData = {
                        eventName:"LeaveRoom",
                        data:"Success"
                    }
                    var toJsonStr = JSON.stringify(resultData);
                    ws.send(toJsonStr);

                console.log("leave room Success");
                }
                else
                {
                    var resultData = {
                        eventName:"LeaveRoom",
                        data:"fail"
                    }
                    var toJsonStr = JSON.stringify(resultData);
                    ws.send(toJsonStr);

                    console.log("leave room fail");
                }
            }
        });     
    }
    console.log('Client Connected.');
    
    wsList.push(ws);

    ws.on("close", ()=>{

        wsList = ArrayRemove(wsList, ws);
        console.log("disconnected.");

        var isLeaveSuccess = false;
                for(var i = 0; i < roomList.length; i++)
                {
                    for(var j = 0; j < roomList[i].wsList.length; j++)
                    {   
                        if(ws == roomList[i].wsList[j])
                        {
                            roomList[i].wsList.splice(j, 1);

                            if(roomList[i].wsList.length <= 0)
                            {
                                roomList.splice(i, 1);
                            }
                            isLeaveSuccess = true;
                            break;
                        }
                    }
                }

                if(isLeaveSuccess)
                {

                    var resultData = {
                        eventName:"LeaveRoom",
                        data:"Success"
                    }
                    var toJsonStr = JSON.stringify(resultData);
                    ws.send(toJsonStr);

                console.log("leave room success");
                }
                else
                {

                    var resultData = {
                        eventName:"LeaveRoom",
                        data:"fail"
                    }
                    var toJsonStr = JSON.stringify(resultData);
                    ws.send(toJsonStr);

                    console.log("leave room fail");
                }
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