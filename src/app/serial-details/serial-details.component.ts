import { Component } from '@angular/core';
import { SerialsService } from '../services/serials.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-serial-details',
  templateUrl: './serial-details.component.html',
  styleUrls: ['./serial-details.component.scss']
})
export class SerialDetailsComponent {

  
  serial:any = {
    "author" : {},
    "chapters" : [{}]
  }
  pageNum:number = 0
  currentPage:number = 1

  descExpanded:boolean = false

  liked:boolean = false

  //TODO delete, fix
  text:any = "Numquam et et voluptatem ab. Ipsum voluptatem ut molestiae consequatur eum voluptate. Culpa possimus officiis ea dolorem. Nisi odio quia eum harum. Aut praesentium quia unde aperiam omnis rerum. Omnis dignissimos dignissimos iure deserunt ducimus et. Sit aut esse quis et repellat sapiente dignissimos et. Corrupti eius ut illo repellendus aliquam voluptatem. Hic molestiae aut illum autem voluptate eum. Delectus ducimus quam voluptatem repudiandae. Beatae eveniet pariatur exercitationem qui doloribus. Sint iste dolores a aut dolorem quis. Dolor officiis repudiandae quia ut qui vero quidem nihil. Ut iure consequatur saepe possimus maxime. Nisi aut dignissimos rerum iusto tenetur quia unde. Animi id vel sit. Quos qui officiis et quas reprehenderit. Quam numquam illo sequi. Quas earum eligendi aut aperiam ut aut. Quis veniam id neque et odit tenetur. Molestiae tenetur expedita cum. Numquam et et voluptatem ab. Ipsum voluptatem ut molestiae consequatur eum voluptate. Culpa possimus officiis ea dolorem. Nisi odio quia eum harum. Aut praesentium quia unde aperiam omnis rerum. Omnis dignissimos dignissimos iure deserunt ducimus et. Sit aut esse quis et repellat sapiente dignissimos et. Corrupti eius ut illo repellendus aliquam voluptatem. Hic molestiae aut illum autem voluptate eum. Delectus ducimus quam voluptatem repudiandae. Beatae eveniet pariatur exercitationem qui doloribus. Sint iste dolores a aut dolorem quis. Dolor officiis repudiandae quia ut qui vero quidem nihil. Ut iure consequatur saepe possimus maxime. Nisi aut dignissimos rerum iusto tenetur quia unde. Animi id vel sit. Quos qui officiis et quas reprehenderit. Quam numquam illo sequi. Quas earum eligendi aut aperiam ut aut. Quis veniam id neque et odit tenetur. Molestiae tenetur expedita cum. Numquam et et voluptatem ab. Ipsum voluptatem ut molestiae consequatur eum voluptate. Culpa possimus officiis ea dolorem. Nisi odio quia eum harum. Aut praesentium quia unde aperiam omnis rerum. Omnis dignissimos dignissimos iure deserunt ducimus et. Sit aut esse quis et repellat sapiente dignissimos et. Corrupti eius ut illo repellendus aliquam voluptatem. Hic molestiae aut illum autem voluptate eum. Delectus ducimus quam voluptatem repudiandae. Beatae eveniet pariatur exercitationem qui doloribus. Sint iste dolores a aut dolorem quis. Dolor officiis repudiandae quia ut qui vero quidem nihil. Ut iure consequatur saepe possimus maxime. Nisi aut dignissimos rerum iusto tenetur quia unde. Animi id vel sit. Quos qui officiis et quas reprehenderit. Quam numquam illo sequi. Quas earum eligendi aut aperiam ut aut. Quis veniam id neque et odit tenetur. Molestiae tenetur expedita cum."

  constructor(private serServ:SerialsService, private route:ActivatedRoute){
    this.getSerial(route.snapshot.paramMap.get("id"))
  }

  //TODO need
  // isLiked(id:any){
  //   this.liked = this.serServ.isLiked(id)
  // }

  getSerial(id:any){
    //TODO delete, fix
    this.serServ.getSerials().subscribe((serial:any) => {
      this.serial = serial[id-1]
      this.pageNum = this.serial.chapters.length % 5 != 0 ? Math.floor(this.serial.chapters.length / 5) + 1 : this.serial.chapters.length / 5
    })
    // this.serServ.getSerial(id).subscribe((serial:any) => {
    //   this.serial = serial
    //   this.pageNum = this.serial.chapters.length % 5 != 0 ? Math.floor(this.serial.chapters.length / 5) + 1 : this.serial.chapters.length / 5
    //   isLiked(this.serial.id)
    // })
  }

  toggleDesc(){
    this.descExpanded = !this.descExpanded
  }

  nextPage(){
    this.currentPage++
  }

  prevPage(){
    this.currentPage--
  }

  jumpPage(page:number){
    this.currentPage = page
  }

  //TODO add redirect if not logged in
  likeSerial(id:any){
    this.serServ.likeSerial(id).subscribe({
      error: ()=> {},
      complete: ()=> {
        this.liked = !this.liked
      }
    })
  }
}
