import { AfterViewInit, ChangeDetectorRef, Component } from '@angular/core';
import { ChaptersService } from '../services/chapters.service';
import { SerialsService } from '../services/serials.service';

@Component({
  selector: 'app-admin-chapters',
  templateUrl: './admin-chapters.component.html',
  styleUrls: ['./admin-chapters.component.css']
})
export class AdminChaptersComponent implements AfterViewInit {

  serials: any[] = [];
  serialIds: any[] = [];

  constructor(private chServ: ChaptersService, private serServ: SerialsService, private changeDT: ChangeDetectorRef) {
    this.getSerials();
  }

  ngAfterViewInit() {
    this.changeDT.detectChanges();
  }

  // getChapters() {
  //   this.chServ.getChapters().subscribe((chapters: any) => {
  //     this.chapters = chapters;
  //     this.groupChaptersBySerial();
  //     this.changeDT.detectChanges();
  //   });
  // }

  groupChapters(serial:any) {
    let processedSerial:any
    processedSerial.lastChapters = serial.chapters.filter((chapter:any) => chapter.isLastChapter);
    processedSerial.otherChapters = serial.chapters.filter((chapter:any) => !chapter.isLastChapter);
    return processedSerial
  }

  deleteChapter(id: number) {
    this.chServ.deleteChapter(id).subscribe(() => this.getSerials());
  }

  updateChapter(chapter: any) {
    chapter.reviewStatus = true;
    this.chServ.putChapter(chapter).subscribe(() => this.getSerials());
  }

  getSerials() {
    this.serServ.getSerials().subscribe((serials: any) => {
      serials.forEach((serial:any) => {
        this.serials.push(this.groupChapters(serial))
      });
      this.changeDT.detectChanges();
    });
  }

  selectSerial(id: any) {
    return this.serials.find((ser: any) => ser.id == id);
  }

}
