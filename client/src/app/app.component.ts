import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { AccountService } from './_services/account.service';
import { User } from './_models/user';

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
    this.getUsers();
    this.setCurrentUser();
  }

  getUsers() {
    this.http.get('http://localhost:5001/api/users').subscribe({
      next: response => this.users = response,
      error: () => { console.log("error") },
      complete: () => { console.log("Request has been completed") }
    })
  }
  setCurrentUser() {
    const user: User = JSON.parse(localStorage.getItem('user')!)
    if(!user){
      return;
    }

    this.accountService.setCurrentUser(user);
  }

}