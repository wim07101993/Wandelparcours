import {Component, OnInit, VERSION} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {Resident} from '../../../../models/resident';
import {MediaService} from '../../../../service/media.service';
import { LoginService } from '../../../../service/login-service.service';

declare var $: any;
declare var Materialize: any;

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

  constructor(private route: ActivatedRoute, private media: MediaService,private login:LoginService) {}

  ngOnInit() {
    this.getAllImages();
    this.typeOfMedia = "image/*";
    this.deleteResidentImage = <Resident>{images: {}};
  }

    /**
     * Reloads the page
     */
  reload() {
    this.getAllImages();
  }

    /**
     * Gets all the URLS for the images
     * @returns {Promise<void>}
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
      Materialize.toast('Media succesvol verwijderd!',5000);
    setTimeout(()=>{
        $("#deleteModal").modal("close");
    }, 200)
    this.getAllImages();
  }

  /**
  *  Closes the modal to delete a picture
  */
  CloseModal() {
    $("#deleteModal").modal("close");
  }

    /**
     * open modal with resident media to delete a picture
     * @param {Resident} resident
     */
  deleteModal(resident: Resident) {
    this.deleteResidentImage = resident;
    $("#deleteModal").modal();
    $("#deleteModal").modal("open");
  }
}
