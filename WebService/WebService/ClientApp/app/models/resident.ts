export class Resident {
    id: string
    firstName: string
    lastName: string
    picture: any
    birthday: Date
    doctor: Doctor
}

export class Doctor {
    name: string
    phoneNumber: string
}