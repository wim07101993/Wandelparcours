import { Component, OnInit } from '@angular/core';
import { RestServiceService } from '../../../../service/rest-service.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-picture',
  templateUrl: './picture.component.html',
  styleUrls: ['./picture.component.css']
})
export class PictureComponent implements OnInit {

    id: string = this.route.snapshot.params['id']
    constructor(private service: RestServiceService, private route: ActivatedRoute) { this.getAllImages(); }


    
    ngOnInit() {}

    async getAllImages() {
        let images: any = await this.service.getImagesOfResidentBasedOnId(this.id);
        console.log(images);
    }


}
