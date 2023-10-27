import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { Message } from 'src/app/_models/Message';
import { MessageService } from 'src/app/_services/message.service';
import { faClock } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { TimeagoModule } from 'ngx-timeago';
import { FormsModule, NgForm } from '@angular/forms';

@Component({
  selector: 'app-member-messages',
  standalone: true,
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css'],
  imports: [CommonModule, FontAwesomeModule, TimeagoModule, FormsModule]
})
export class MemberMessagesComponent implements OnInit{
  
  @ViewChild('messageForm') messageForm?: NgForm
  @Input() username?: string;

  @Input() messages: Message[] = [];

  messageContent = '';

  faclock = faClock;
  constructor(private messageService: MessageService) {
  
  }
  
  ngOnInit(): void {
 
  }

  sendMessage(){
    if(!this.username){
      return;
    } 

    this.messageService.sendMessage(this.username, this.messageContent).subscribe({
      next: message=>{
        this.messages.push(message);
        this.messageForm?.reset();
      }
    })
  }

  

  
}
