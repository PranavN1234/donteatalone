import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { Member } from 'src/app/_models/Member';
import { faHeart,faUser, faEnvelope } from '@fortawesome/free-solid-svg-icons';
import { MembersService } from 'src/app/_services/members.service';
import { ToastrService } from 'ngx-toastr';
import { PresenceService } from 'src/app/_services/presence.service';
@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit{
  @Input() member: Member|undefined;
  faHeart = faHeart;
  faUser = faUser;
  faEnvelope = faEnvelope;
  

  constructor(private memberService: MembersService, private toastr: ToastrService, public presenceService: PresenceService) {
    
    
  }
  ngOnInit(): void {
  
  }

  addLike(member: Member){
    this.memberService.addLike(member.userName).subscribe({
      next: ()=>{
        this.toastr.success('Liked '+member.knownas)
      }
    })
  }

  
}
