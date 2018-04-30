import * as fs from "fs";
import { ChatServer } from "./serversocket";

let clientCode= fs.readFileSync("./client.js","utf8");


let chatserver= new ChatServer(clientCode)
//console.log(clientside);