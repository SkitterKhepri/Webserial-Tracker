import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { HomeComponent } from './home/home.component';
import { SearchComponent } from './search/search.component';
import { HttpClientModule } from '@angular/common/http';
import { NavBarComponent } from './nav-bar/nav-bar.component';
import { LogInComponent } from './log-in/log-in.component';
import { RegistrationComponent } from './registration/registration.component';
import { AdminSerialsComponent } from './admin-serials/admin-serials.component';
import { AdminUsersComponent } from './admin-users/admin-users.component';
import { FormsModule } from '@angular/forms';
import { ResultsComponent } from './results/results.component';
import { NewSerialComponent } from './new-serial/new-serial.component';
import { AdminChaptersComponent } from './admin-chapters/admin-chapters.component';
import { AdminNewserialComponent } from './admin-newserial/admin-newserial.component';
import { LikedSerialsComponent } from './liked-serials/liked-serials.component';
import { PasswordResetComponent } from './password-reset/password-reset.component';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    SearchComponent,
    NavBarComponent,
    LogInComponent,
    RegistrationComponent,
    AdminSerialsComponent,
    AdminUsersComponent,
    ResultsComponent,
    NewSerialComponent,
    AdminChaptersComponent,
    AdminNewserialComponent,
    LikedSerialsComponent,
    PasswordResetComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    NgbModule,
    HttpClientModule,
    FormsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
