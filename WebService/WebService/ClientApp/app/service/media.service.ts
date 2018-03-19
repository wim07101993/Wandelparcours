import { Injectable } from "@angular/core";
import { RestServiceService } from "./rest-service.service";
import { Resident } from "../models/resident";

@Injectable()
export class MediaService {
    fullLinks: any = [];
    check: any;
    url: any = "http://localhost:5000/api/v1/media/";
    constructor(private service: RestServiceService) { }

    async getMedia(id: string, media: string) {
        this.fullLinks = [];
        let mediaType = await this.service.getCorrectMediaOfResidentBasedOnId(id, media);
        for (let a of mediaType) {
            let url2 = this.url + a.id;
            let fullLinks = new Resident();

            if (media === "/images") {
                fullLinks.images.id = a.id;
                fullLinks.images.url = url2;
                this.fullLinks.push(fullLinks);
            }
            else if (media === "/videos") {
                fullLinks.videos.id = a.id;
                fullLinks.videos.url = url2;
                this.fullLinks.push(fullLinks);
            } 
         }
        return this.fullLinks;
    }

    async deleteMedia(id: string, mediaId: string, media: string) {
        return this.check = await this.service.deleteResidentMediaByUniqueId(id, mediaId, media);
        
    }
}