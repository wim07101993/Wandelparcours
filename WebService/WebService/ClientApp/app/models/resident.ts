export class Resident {
    id: string
    firstName: string
    lastName: string
    picture: any
    room: string
    birthday: Date
    doctor: Doctor
    images: Images
    videos: Videos
    constructor() {
        this.images = new Images();
        this.id = "";
        this.firstName = "";
        this.lastName = "";
        this.room = "";
        this.picture = "";
        this.birthday = new Date();
        this.doctor = new Doctor();
        this.videos = new Videos();
    }
}

export class Doctor {
    name: string
    phoneNumber: string
    constructor() {
        this.name = "";
        this.phoneNumber = "";
    }
}

export class Images {
    id: string
    url: string
    name: string

    constructor() {
        this.id = "";
        this.url = "";
        this.name = "";
    }
}

export class Videos {
    id: string
    url: string
    type: string
    name: string

    constructor() {
        this.id = "";
        this.url = "";
        this.type = "";
        this.name = "";
    }
}