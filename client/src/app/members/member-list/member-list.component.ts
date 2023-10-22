import { Component, OnInit } from '@angular/core';
import { Observable, take } from 'rxjs';
import { Member } from 'src/app/_models/Member';
import { Pagination } from 'src/app/_models/Pagination';
import { userParams } from 'src/app/_models/UserParams';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: [ './member-list.component.css' ]
})
export class MemberListComponent implements OnInit {

  // members$: Observable<Member[]>|undefined; 
  members: Member[] = [];
  pagination: Pagination | undefined;
  userParams: userParams | undefined;


  genderlist = [ { value: 'male', display: 'Males' }, { 'value': "female", display: 'Females' }, { 'value': 'Non Binary', display: 'Non Binary' } ]
  constructor(private memberService: MembersService) {
    this.userParams = this.memberService.getUserParams();

  }
  ngOnInit(): void {
    this.loadMembers();
  }

  loadMembers() {

    if (this.userParams) {
      this.memberService.setUserParams(this.userParams);
      this.memberService.getMembers(this.userParams).subscribe({
        next: response => {
          if (response.result && response.pagination) {
            this.members = response.result;
            this.pagination = response.pagination;
          }
        }
      })
    }

  }

  resetFilters() {
    console.log("inside reset");
    this.userParams = this.memberService.resetUserParams();
    this.loadMembers();
  }
  pageChanged(event: any) {

    if (this.userParams && this.userParams?.pageNumber !== event.page) {
      this.userParams.pageNumber = event.page;
      this.memberService.setUserParams(this.userParams);
      this.loadMembers();
    }
  }

}
