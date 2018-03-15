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
}

export class Doctor {
    name: string
    phoneNumber: string
}

export class Images {
    id: string
    url: string
}

export class Videos {
    id: string
    url: string
}