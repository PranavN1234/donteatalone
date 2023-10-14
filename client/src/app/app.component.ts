import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { AccountService } from './_services/account.service';
import { User } from './_models/user';
import { environment } from 'src/environments/environment.development';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: [ './app.component.css' ]
})
export class AppComponent implements OnInit {
  title = "Don't eat alone";
  users: any;

  constructor(private http: HttpClient, private accountService: AccountService) {

  }
  ngOnInit(): void {
   
    this.setCurrentUser();
  }


  setCurrentUser() {
    const user: User = JSON.parse(localStorage.getItem('user')!)
    if(!user){
      return;
    }

    this.accountService.setCurrentUser(user);
  }

}
