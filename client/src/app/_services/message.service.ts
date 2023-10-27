import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { GetPaginatedResult, getPaginationHeaders } from './pagination-helper';
import { Message } from '../_models/Message';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient) { }

  getMessages(pageNumber: number, pageSize: number, container: string){
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('Container', container);
    return GetPaginatedResult<Message []>(this.baseUrl+'messages', params, this.http);
  }

  getMessageThread(username: string){
    return this.http.get<Message []>(this.baseUrl+'messages/threads/'+username);
  }

  sendMessage(username: string, content: string){
    return this.http.post<Message>(this.baseUrl+'messages', {receipientUsername: username, content});
  }

  deleteMessage(id: number){
    return this.http.delete(this.baseUrl+'messages/'+id);
  }
}
