import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import {Member} from '../_models/Member';
import { map, of, take } from 'rxjs';
import { PaginatedResult } from '../_models/Pagination';
import { userParams } from '../_models/UserParams';
import { AccountService } from './account.service';
import { User } from '../_models/user';
import { GetPaginatedResult, getPaginationHeaders } from './pagination-helper';
@Injectable({
  providedIn: 'root'
})
export class MembersService {

  baseUrl = environment.apiUrl;
  members: Member[] = [];
  memberCache = new Map();
  user: User|undefined;
  userParams: userParams|undefined;
   
  constructor(private http: HttpClient, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user=>{
        if(user){
          this.userParams = new userParams(user);
          this.user = user;
        }
      }
    })
   }
   getUserParams(){
    return this.userParams;
   }

   setUserParams(params: userParams){
    this.userParams = params;
   }

   resetUserParams(){
    if(this.user){
      console.log("resetting params...");
      this.userParams = new userParams(this.user);
    
      return this.userParams;
    }
    return;
   }
   getMembers(userParams: userParams){

    const response = this.memberCache.get(Object.values(userParams).join('-'));

    if(response) return of(response);

    let params =  getPaginationHeaders(userParams.pageNumber, userParams.pageSize);
    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);
    return GetPaginatedResult<Member []>(this.baseUrl+'users', params, this.http).pipe(map(response=>{
      this.memberCache.set(Object.values(userParams).join('-'), response);
      return response;
    }));
   }


   getMember(username: string){
    
    const member = [...this.memberCache.values()].reduce((arr, element)=> arr.concat(element.result), []).find((member: Member)=> member.userName === username);

    if (member) return of(member);

    return this.http.get<Member>(this.baseUrl+'users/'+username);
   }

   updateMember(member: Member){
      return this.http.put(this.baseUrl+'users', member).pipe(map(()=>{
        const indx = this.members.indexOf(member);
        this.members[indx] = {...this.members[indx], ...member}
      }));
   }

   setMainPhoto(photoId: number){
      return this.http.put(this.baseUrl+'users/set-main-photo/'+photoId, {});
   }

   deletePhoto(photoId: number){
      return this.http.delete(this.baseUrl+'users/delete-photo/'+photoId);
   }

   addLike(username: string){
    return this.http.post(this.baseUrl+'likes/'+username, {});
   }

   getLikes(predicate: string, pageNumber: number, pageSize: number){

    let params = getPaginationHeaders(pageNumber, pageSize);

    params = params.append('predicate', predicate);


    return GetPaginatedResult<Member []>(this.baseUrl+'likes', params, this.http);
   }


}
