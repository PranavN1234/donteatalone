import { Component, Input, ViewEncapsulation } from '@angular/core';
import { Member } from 'src/app/_models/Member';
import { faHeart,faUser, faEnvelope } from '@fortawesome/free-solid-svg-icons';
@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent {
  @Input() member: Member|undefined;
  faHeart = faHeart;
  faUser = faUser;
  faEnvelope = faEnvelope;

  /**
   *
   */
  constructor() {
    
    
  }

  
}
