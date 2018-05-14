import {Injectable} from '@angular/core';
import {RestServiceService} from './rest-service.service';
import {Resident} from '../models/resident';

@Injectable()
export class MediaService {
  fullLinks: any = [];
  /**
   *@ignore
   */
  get url(): string {
    //return 'http://localhost:5000/api/v1/media/';
     return document.getElementsByTagName('base')[0].href + 'api/v1/media/';
  }

  constructor(private service: RestServiceService) {}

  /**
   * Get correct media of resident based on id and media type
   * @param id resident id
   * @param media media type
   * converts all images to url and sends this back
   */
  async getMedia(id: string, media: string) {
    this.fullLinks = [];
    let mediaType = await this.service.getCorrectMediaOfResidentBasedOnId(id, media);
    for (const a of mediaType) {
      const url2 = this.url + a.id + '/file';
      const url3 = this.url + a.id + '/file';
      const fullLinks = new Resident();

      if (media === '/images') {
        fullLinks.images.id = a.id;
        fullLinks.images.url = url2;
        this.fullLinks.push(fullLinks);
      } else if (media === '/videos') {
        fullLinks.videos.id = a.id;
        fullLinks.videos.url = url2;
        this.fullLinks.push(fullLinks);
      }
      else if(media === '/music'){
        fullLinks.music.id = a.id;
        fullLinks.music.url = url3 + "." + a.extension;
        fullLinks.music.extension = a.extension;
        fullLinks.music.title = a.title;
        this.fullLinks.push(fullLinks);
      }
    }
    return this.fullLinks;
  }

  /**
   * Delete media of resident based on id mediaId and media
   * @param id resident id
   * @param mediaId resident media id
   * @param media media type
   * returns true or false
   */
  async deleteMedia(id: string, mediaId: string, media: string) {
    await this.service.deleteResidentMediaByUniqueId(id, mediaId, media);
  }
}
