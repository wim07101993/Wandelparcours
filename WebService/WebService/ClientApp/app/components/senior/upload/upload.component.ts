import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { RestServiceService } from '../../../service/rest-service.service';
import { ActivatedRoute, Router } from '@angular/router';
import { RequestOptions } from '@angular/http';

declare var $: any;

@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.css']
})
export class UploadComponent implements OnInit {
    id: string = this.route.snapshot.params['id'];
    @Input() type: string;
    fd: FormData;
    selectedFile: any;
    loading: string = "";
    check: any;
    addPicture: string = "/images/data";
    addVideo: string = "/videos/data";
    @Output() reload = new EventEmitter();

    constructor(private restService: RestServiceService, private route: ActivatedRoute, private router: Router) {}

    ngOnInit() {
    }

    /**
     * Observer event if anything changes
     * @param event
     */
      onFileSelected(event: any) {
          this.loading = "Upload"
          this.selectedFile = <any>event.target.files;   
      }

    /**
     * Upload selected file as formdata either to image or video depeping on this.selectefFile[index].type --> image or video
     * Loop trough all selectedfiles
     * 
     */
      async onUpload() {
          for (const file in this.selectedFile) {
              const index = parseInt(file);
              if (!isNaN(index)) {
                  this.loading = "uploading...";
                  const fd = new FormData();
                  fd.append("File", this.selectedFile[index], this.selectedFile[index].name);
                  if (this.selectedFile[index].type.indexOf("image") != -1) {
                      this.check = await this.restService.addCorrectMediaToDatabase(this.id, fd, this.addPicture);
                  }
                  else if (this.selectedFile[index].type.indexOf("video") != -1) {
                      this.check = await this.restService.addCorrectMediaToDatabase(this.id, fd, this.addVideo);
                  }
                  else{
                      alert("won't work");
                  }
              
              }
              
              

              $("#add-picture").modal("close");
          
          }
          //clear selected files
          this.selectedFile = [];
          if (this.check) {
              $(".preview").empty();
              this.reload.emit();
              this.loading = "Uploaded!"
          } else{
              this.router.navigate(["/error"]);
          }

        $(".preview").html("<p>Momenteel geen bestanden geselecteerd</p>");

          
      }
      

        /**
         *
         * Open modal in edit mode and fill modal with resident
         *
         */
        addPhotoModal(){
            $("#add-picture").modal();
            $("#add-picture").modal("open");
        }
        
}
