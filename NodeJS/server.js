var websocket = require('ws');
const sqlite3 = require('sqlite3').verbose();

var websocketServer = new websocket.Server({port:36500},()=> {
    console.log("DOme Test Server is running");
});

let db = new sqlite3.Database('./databaseOnserver/ChatDB.db', sqlite3.OPEN_CREATE | sqlite3.OPEN_READWRITE, (err)=>{
    if(err) throw err;

    console.log('Connected to database.');

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
                        data: "Success",
                        idUser: "",
                        userName:toJson.data
                    }

                    var toJsonStr = JSON.stringify(resultData)

                    ws.send(toJsonStr);
                }
            }

            else if(toJson.eventName == "Login")
            {
                var sqlSelect = "SELECT * FROM UserDataHomeWork WHERE UserID = '"+toJson.idUser+"' AND Password = '"+toJson.passWord+"'";
                db.all(sqlSelect, (err, rows)=>{
                    console.log(rows);
                    if(err) //ถ้า Erorr 
                    {
                        /*var callbackMsg = {
                            eventName: "Login",
                            data:"fail"
                        }*/
                        console.log(err);
                        //var toJsonStr = JSON.stringify(callbackMsg);
                        console.log("[1]") +toJsonStr
                        //ws.send(toJsonStr);
                    }
                    else //ไม่ Erorr
                    {
                        if(rows.length>0)
                        {
                            var callbackMsg = {
                                eventName: "Login",
                                data:"Success",
                                userName:rows[0].Name
                            }
                
                            var toJsonStr = JSON.stringify(callbackMsg);
                            console.log("[2]") +toJsonStr
                            ws.send(toJsonStr);
                        }
                        else
                        {
                            var callbackMsg = {
                                eventName: "Login",
                                data:"fail"
                            }

                            var toJsonStr = JSON.stringify(callbackMsg);
                            console.log("[3]") +toJsonStr
                            ws.send(toJsonStr);
                        }                                             
                    }
                })

            }

            else if(toJson.eventName == "Regiter")
            {
                var sqlInsert = "INSERT INTO UserDataHomeWork (UserID,Name,Password)  VALUES ('"+toJson.idUser+"', '"+toJson.userName+"','"+toJson.passWord+"')";
                db.all(sqlInsert, (err, rows)=>{
                    if(err) //ถ้า Erorr 
                    {
                        var callbackMsg = {
                            eventName: "Regiter",
                            data:"fail",
                            idUser: ""
                        }
            
                        var toJsonStr = JSON.stringify(callbackMsg);
                        console.log("[0]") +toJsonStr
                        ws.send(toJsonStr);
                    }
                    else //Regiter Success
                    {
                        var callbackMsg = {
                            eventName: "Regiter",
                            data:"Success",
                            idUser: "",
                            userName:toJson.userName
                        }
            
                        var toJsonStr = JSON.stringify(callbackMsg);
                        console.log("[0]") +toJsonStr
                        ws.send(toJsonStr);
                    }
                })

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
                        data: "Success",
                        idUser: "",
                        userName:toJson.data
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
                        data: "fail",
                        idUser: "",
                        userName:toJson.data
                        
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
            else if(toJson.eventName == "SendMessage")
            {
                Boardcast(ws,data)
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

function Boardcast(ws,data)
{
    var selectRoomIndex = -1;

    for(var i = 0; i < roomList.length;i++)
    {
        for(var j = 0; j < roomList[i].wsList.length;j++)
        {
            if(ws == roomList[i].wsList[j])
            {
                selectRoomIndex = i;
            }
        }
    }
    for(var i = 0; i < roomList[selectRoomIndex].wsList.length; i++)
    {

        roomList[selectRoomIndex].wsList[i].send(data)
    }
}


/////////////////////////////////////////////////





    //var splitStr = dataFromClient.data.split('#');
    /*var userID = splitStr[0];
    var password = splitStr[1];
    var name = splitStr[2]*/

    /*var sqlInsert = "INSERT INTO UserDataHomeWork (UserID, Password, Name) VALUES ('"+userID+"', '"+password+"', '"+name+"')"; //register

    db.all(sqlInsert, (err, rows)=>{  //register
        if(err)
        {
            var callbackMsg = {
                eventName: "Regiter",
                data:"fali"
            }

            var toJsonStr = JSON.stringify(callbackMsg);
            console.log("[0]" +toJsonStr)
        }
        else
        {
            var callbackMsg = {
                eventName: "Regiter",
                data:"success"
            }

            var toJsonStr = JSON.stringify(callbackMsg);
            console.log("[1]" +toJsonStr)  
        }

    });
    var sqlSelect = "SELECT  * FROM UserData WHERE UserID = '"+userID+"' , AND Password = '"+password+"'"; //Login

    db.all(sqlSelect, (err, rows)=>{   //Login
        if(err)
        {
            console.log("[0]" + err);
        }
        else
        {
            if(rows.length > 0)
            {
                console.log("======[1]======")
                console.log(rows);
                console.log("======[1]======")
                var callbackMsg = {
                    eventName:"Login",
                    data:"success"
                }

                var toJsonStr = JSON.stringify(callbackMsg);
                console.log("[2]" +toJsonStr) 
            }

            else
            {
                var callbackMsg = {
                    eventName: "Login",
                    data:"fali"
                }

                var toJsonStr = JSON.stringify(callbackMsg);
                console.log("[3]" +toJsonStr) 
            }           
        }
    })
})*/