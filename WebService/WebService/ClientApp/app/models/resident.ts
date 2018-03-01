export class Resident {
    id: string
    firstName: string
    lastName: string
    picture: any
    room: string
    birthday: Date
    doctor: Doctor
}

export class Doctor {
    name: string
    phoneNumber: string
}