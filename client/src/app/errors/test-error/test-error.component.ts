import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-test-error',
  templateUrl: './test-error.component.html',
  styleUrls: ['./test-error.component.css']
})
export class TestErrorComponent implements OnInit{

  baseUrl = "http://localhost:5000/api/";
  validationErrors: string [] = [];

  constructor(private http: HttpClient){

  }
  ngOnInit(): void {
  }

  get404Error(){
    this.http.get(this.baseUrl+'some/not-found').subscribe({
      next:response=>{console.log(response)},
      error: error=>{console.log(error)}
    })
  }

  get400Error(){
    this.http.get(this.baseUrl+'some/bad-request').subscribe({
      next:response=>{console.log(response)},
      error: error=>{console.log(error)}
    })
  }

  get500Error(){
    this.http.get(this.baseUrl+'some/server-error').subscribe({
      next:response=>{console.log(response)},
      error: error=>{console.log(error)}
    })
  }

  get401Error(){
    this.http.get(this.baseUrl+'some/auth').subscribe({
      next:response=>{console.log(response)},
      error: error=>{console.log(error)}
    })
  }
  get400ValidationError(){
    this.http.post(this.baseUrl+'account/register', {}).subscribe({
      next:response=>{console.log(response)},
      error: error=>{console.log(error);

        this.validationErrors = error;
      
      
      }
    })
  }

}
