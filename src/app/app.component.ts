import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'Webserial_tracker';

  eventFuck(sent:any){
    console.log(sent)
  }
}
