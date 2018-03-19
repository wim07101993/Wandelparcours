import { Component, OnInit, ViewChild, Injectable, Input } from '@angular/core';
import { RestServiceService } from '../../../../service/rest-service.service';
import { ActivatedRoute, Router } from '@angular/router';
import { RequestOptions, Headers } from '@angular/http';
import { UploadComponent } from '../../upload/upload.component';
import { Resident } from '../../../../models/resident';
import { MediaService } from '../../../../service/media.service';
declare var $: any;


@Component({
    selector: 'app-picture',
    templateUrl: './picture.component.html',
    styleUrls: ['./picture.component.css'],
})
export class PictureComponent implements OnInit {
    typeOfMedia: string;
    picture: string = "/images";
    check: any;
    ngOnInit() { }

    images: Resident[];
    fullLinks: any=[];
    id: string = this.route.snapshot.params['id'];
    selectedFile: File;

    constructor(private route: ActivatedRoute, private router: Router, private media: MediaService) {
        this.getAllImages();
        this.typeOfMedia = "image/*";
    }

    reload() {
        this.getAllImages();
    }

    async getAllImages() {
        this.fullLinks = [];
        this.fullLinks = await this.media.getMedia(this.id, this.picture);
        //console.log(this.fullLinks);
    }

    async deleteResidentMediaByUniqueId(uniquePictureID: string) {
        this.check = await this.media.deleteMedia(this.id, uniquePictureID, this.picture);
        if (this.check) {
            this.getAllImages();
        } else {
            this.router.navigate(["/error"]);
        }

    }
}