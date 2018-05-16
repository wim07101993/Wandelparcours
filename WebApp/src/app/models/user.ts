export class user{
    id: string;
    userName: string;
    userType: string;
    email: string
    group: string
    resident: any;
    password: any;

    constructor(){
        this.id="";
        this.userName="";
        this.userType="";
        this.email="";
        this.group="";
        this.resident={};
        this.password="";
    }
}