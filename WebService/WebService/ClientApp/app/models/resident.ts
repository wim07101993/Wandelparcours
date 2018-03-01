export class Resident {
    id: string
    firstName: string
    lastName: string
    picture: any
    birthDay: string
    doctor: Doctor
}

export class Doctor {
    name: string
    phoneNumber: string
}