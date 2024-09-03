import { AfterViewInit, ChangeDetectorRef, Component } from '@angular/core';
import { ChaptersService } from '../services/chapters.service';
import { SerialsService } from '../services/serials.service';

@Component({
  selector: 'app-admin-chapters',
  templateUrl: './admin-chapters.component.html',
  styleUrls: ['./admin-chapters.component.css']
})
export class AdminChaptersComponent implements AfterViewInit {

  chapters: any[] = [];
  serials: any[] = [];
  chaptersGroupedBySerial: any = {};
  serialIds: any[] = [];

  constructor(private chServ: ChaptersService, private serServ: SerialsService, private changeDT: ChangeDetectorRef) {
    this.getChapters();
    this.getSerials();
  }

  ngAfterViewInit() {
    this.changeDT.detectChanges();
  }

  getChapters() {
    this.chServ.getChapters().subscribe((chapters: any) => {
      this.chapters = chapters;
      this.groupChaptersBySerial();
      this.changeDT.detectChanges();
    });
  }

  groupChaptersBySerial() {
    this.chaptersGroupedBySerial = {};

    this.chapters.forEach(chapter => {
      const serialId = chapter.serialId;
      if (!this.chaptersGroupedBySerial[serialId]) {
        this.chaptersGroupedBySerial[serialId] = 
        {
          lastChapters : [],
          otherChapters : []
        };
      }
      if(chapter.isLastChapter){
        this.chaptersGroupedBySerial[serialId].lastChapters.push(chapter);
      }
      else{
        this.chaptersGroupedBySerial[serialId].otherChapters.push(chapter);
      }
      
    });
    this.serialIds = Object.keys(this.chaptersGroupedBySerial);
  }

  deleteChapter(id: number) {
    this.chServ.deleteChapter(id).subscribe(() => this.getChapters());
  }

  updateChapter(chapter: any) {
    chapter.reviewStatus = true;
    this.chServ.putChapter(chapter).subscribe(() => this.getChapters());
  }

  getSerials() {
    this.serServ.getSerials().subscribe((serials: any) => {
      this.serials = serials;
      this.changeDT.detectChanges();
    });
  }

  selectSerial(id: any) {
    return this.serials.find((ser: any) => ser.id == id);
  }

}
