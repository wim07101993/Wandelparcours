import { Component, OnInit, ViewChild, Injectable } from '@angular/core';
import { RestServiceService } from '../../../../service/rest-service.service';
import { ActivatedRoute } from '@angular/router';
import { RequestOptions, Headers } from '@angular/http';
declare var $: any;


@Component({
  selector: 'app-picture',
  templateUrl: './picture.component.html',
  styleUrls: ['./picture.component.css'],
})
export class PictureComponent implements OnInit {
    fd: FormData = new FormData();
    ngOnInit() { }


    id: string = this.route.snapshot.params['id'];
    selectedFile: File;

    constructor(private service: RestServiceService, private route: ActivatedRoute, ) {
        this.getAllImages();
    }

    onFileSelected(event: any) {
        this.selectedFile = <File>event.target.files[0];
        console.log(this.selectedFile);
    }

    async onUpload() {
        this.fd.append('images/data', this.selectedFile, this.selectedFile.name);
        console.log(this.fd); console.log(this.selectedFile.type);
        //let headers = new Headers();
        //let options = new RequestOptions({ headers: headers });    
        //await this.service.addImagesToDatabase(this.id, new Blob([JSON.stringify(this.selectedFile.name)], { type: "application/json"}));
        let images = this.service.addImagesToDatabase(this.id, this.fd);
        //console.log(images);
    }    

    async getAllImages() {
        let images: any = await this.service.getImagesOfResidentBasedOnId(this.id);
        console.log(images);
    }
}