import {Component, OnInit, VERSION} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {Resident} from '../../../../models/resident';
import {MediaService} from '../../../../service/media.service';


declare var $: any;


@Component({
  selector: 'app-picture',
  templateUrl: './picture.component.html',
  styleUrls: ['./picture.component.css'],
})
export class PictureComponent implements OnInit {
  typeOfMedia: string;
  picture: string = "/images";
  deleteResidentImage: Resident;
  images: Resident[];
  fullLinks: any=[];
  id: string = this.route.snapshot.params['id'];
    name: string;

  constructor(private route: ActivatedRoute, private media: MediaService) {}



  ngOnInit() {
    this.getAllImages();
    this.typeOfMedia = "image/*";
    this.deleteResidentImage = <Resident>{images: {}};
  }
  /**
   * reload page
   */
  reload() {
    this.getAllImages();
  }

  /**
   * Gets all urls for images
   */
  async getAllImages() {
    this.fullLinks = [];
    this.fullLinks = await this.media.getMedia(this.id, this.picture);
  }

  /**
   * Delete resident media based on uniqueId
   * @param uniquePictureID unique pictureId
   * Either reloads the page or sends user to errorpage
   */
  async deleteResidentMediaByUniqueId(uniquePictureID: string) {
    await this.media.deleteMedia(this.id, uniquePictureID, this.picture);
    setTimeout(()=>{
        $("#deleteModal").modal("close");
    }, 200)
    this.getAllImages();
  }

  /*
  *   Closes the modal to delete a picture
  */
  CloseModal() {
    $("#deleteModal").modal("close");
  }

  /*
  *   Opens modal to delete a picture
  */
  deleteModal(resident: Resident) {
    this.deleteResidentImage = resident;
    $("#deleteModal").modal();
    $("#deleteModal").modal("open");
  }
}
