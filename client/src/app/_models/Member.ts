import { Photo } from "./Photo"

export interface Member {
    id: number
    userName: string
    photoUrl: any
    age: number
    knownas: string
    created: string
    lastActive: string
    gender: string
    introduction: string
    cuisines: string
    city: string
    country: string
    photos: Photo[]
  }
  
