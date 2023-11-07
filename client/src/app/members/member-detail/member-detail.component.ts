import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabDirective, TabsModule, TabsetComponent } from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
import { ToastrModule, ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/_models/Member';
import { MembersService } from 'src/app/_services/members.service';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { MessageService } from 'src/app/_services/message.service';
import { Message } from 'src/app/_models/Message';
import { PresenceService } from 'src/app/_services/presence.service';

import { faUserCircle } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { AccountService } from 'src/app/_services/account.service';
import { User } from 'src/app/_models/user';
import { take } from 'rxjs';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  standalone: true,
  styleUrls: [ './member-detail.component.css' ],
  imports: [ CommonModule, TabsModule, GalleryModule, TimeagoModule, ToastrModule, MemberMessagesComponent, FontAwesomeModule]
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  @ViewChild('memberTabs', {static: true}) memberTabs?: TabsetComponent;
  member: Member = {} as Member;
  images: GalleryItem[] = [];
  activeTab?: TabDirective;
  messages: Message[]=[];
  faUserCircle = faUserCircle;
  user?: User;
  ngOnInit(): void {
    this.route.data.subscribe({
      next: data=>this.member = data['member']
    })

    this.route.queryParams.subscribe({
      next: params=>{
        params['tab'] && this.selectTab(params['tab'])
      }
    })

    this.getImages()
  }

  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }

  onTabActivated(data: TabDirective){
    this.activeTab = data;
    if(this.activeTab.heading == "Messages" && this.user){
      this.messageService.createHubConnection(this.user, this.member.userName);
    }else{
      this.messageService.stopHubConnection();
    }
  }
  loadMessages(){
    if(this.member){
      this.messageService.getMessageThread(this.member.userName).subscribe({
        next: response=>{
          this.messages = response;
        }
      })
    }
  }

  selectTab(heading: string){
    if(this.memberTabs){
      this.memberTabs.tabs.find(x=>x.heading === heading)!.active = true;
    }
  }
  constructor(private memberService: MembersService, private accountService: AccountService, private route: ActivatedRoute,private toastr: ToastrService, private messageService: MessageService, public presenceService: PresenceService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user=>{
        if(user){
          this.user = user;
        }
      }
    })
  }



  getImages() {
    if (!this.member) return;
    for (const photo of this.member?.photos) {
      this.images.push(new ImageItem({ src: photo.url, thumb: photo.url }));
    }
  }
  
  addLike(member: Member){
    this.memberService.addLike(member.userName).subscribe({
      next: ()=>{
        this.toastr.success('Liked '+member.knownas)
      }
    })
  }

}
