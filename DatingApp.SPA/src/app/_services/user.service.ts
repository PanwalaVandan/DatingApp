import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { PaginatedResult } from '../_models/Pagination';
import { map } from 'rxjs/operators';
import { Message } from '../_models/Message';

const httpOptions = {
  headers: new HttpHeaders({
    'Authorization': 'Bearer ' + localStorage.getItem('token')
  })
};

@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getUsers(page?, itemsPerPage?, userParams?, likesParam?): Observable<PaginatedResult<User[]>> {
    const paginatedResult: PaginatedResult<User[]> = new PaginatedResult<User[]>();
    let params = new HttpParams();

    if (page != null && itemsPerPage != null) {
      // Appending parameters to the http params
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);

    }

    if (userParams != null) {
      params = params.append('minAge', userParams.minAge);
      params = params.append('maxAge', userParams.maxAge);
      params = params.append('gender', userParams.gender);
      params = params.append('orderBy', userParams.orderBy);
      // params = params.append('minAge', userParams.minAge);
    }

    if (likesParam === 'Likers') {
      params = params.append('likers', 'true');
    }

    if (likesParam === 'Likees') {
      params = params.append('likees', 'true');
    }
    return this.http.get<User[]>(this.baseUrl + 'users', { observe: 'response', params})
    // doing something of response with the pipe operator
    .pipe(
      map(response => {
        paginatedResult.result = response.body;
        if (response.headers.get('Pagination') != null) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return paginatedResult;
      })
    );
  }

  getUser(id): Observable<User> {
    return this.http.get<User>(this.baseUrl + 'users/' + id, httpOptions);
  }

  updateUser(id: number, user: User) {
    return this.http.put(this.baseUrl + 'users/' + id, user);
  }

  setmainPhoto(userId: number, id: number) {
    return this.http.post(this.baseUrl + 'users/' + userId + '/photos/' + id + '/setMain', {});
  }

  deletePhoto(userId: number, id: number) {
    return this.http.delete(this.baseUrl + 'users/' + userId + '/photos/' + id);
  }

  sendLike(id: number, recipientId: number) {
    return this.http.post(this.baseUrl + 'users/' + id + '/like/' + recipientId, {});
  }

  getMessages(id: number, page?, itemsPerPage?, messageContainer?) {
    const paginatedResult: PaginatedResult<Message[]>  = new PaginatedResult<Message[]>();
    let params = new HttpParams();
    params = params.append('MessageContainer', messageContainer);

    if (page != null && itemsPerPage != null) {
      // Appending parameters to the http params
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);

    }

    return this.http.get<Message[]>(this.baseUrl + 'users/' + id + '/messages', {observe: 'response', params})
    .pipe(
      map(response => {
        paginatedResult.result = response.body;
        if (response.headers.get('Pagination') !== null) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }

        return paginatedResult;
      })
    );
  }

  getMessageThread(id: number, recipientId: number) {
    return this.http.get<Message[]>(this.baseUrl + 'users/' + id + '/messages/thread/' + recipientId);
  }

  sendMessage(id: number, message: Message) {
    return this.http.post(this.baseUrl + 'users/' + id + '/messages', message);
  }

  deleteMessage(id: number, userId: number) {
    return this.http.post(this.baseUrl + 'users/' + userId + '/messages/' + id, {});
  }

  markAsRead(userId: number, messageId: number) {
    return this.http.post(this.baseUrl + 'users/' + userId + '/messages/' + messageId + '/read', {})
    .subscribe();
  }
}
// import { Injectable } from '@angular/core';
// import { environment } from '../src/environments/environment';
// import { Http, RequestOptions, Headers } from '@angular/http';
// // tslint:disable-next-line:import-blacklist
// import { Observable } from 'rxjs/Rx';
// import { User } from '../src/app/_models/User';
// import 'rxjs/add/operator/map';
// import 'rxjs/add/operator/catch';
// import 'rxjs/add/observable/throw';
// import { AuthHttp } from 'angular2-jwt';

// @Injectable()
// export class UserService {
//     baseUrl = environment.apiUrl;
//     constructor(private authHttp: AuthHttp) { }


// getUsers(): Observable<User[]> {
//     return this.authHttp
//     .get(this.baseUrl + 'users')
//     .map(response => <User[]>response.json())
//     .catch(this.handleError);
// }

// getUser(id): Observable<User> {
//     return this.authHttp
//     .get(this.baseUrl + 'users/' + id)
//     .map(response => <User>response.json())
//     .catch(this.handleError);
// }

// // // method to get the token from local storage for authentication
// // private jwt() {
// //     // tslint:disable-next-line:prefer-const
// //     let token = localStorage.getItem('token');
// //     if (token) {
// //         // tslint:disable-next-line:prefer-const
// //         let headers = new Headers({'Authorization': 'Bearer ' + token});
// //         headers.append('Content-type', 'application/json');
// //         return new RequestOptions({headers: headers});
// //     }
// // }

// private handleError(error: any) {
//     const applicationError = error.headers.get('Application-Error');
//     if (applicationError) {
//         return Observable.throw(applicationError);
//     }

//     const serverError = error.json();
//     let modelStateError = '';
//     if (serverError) {
//         for (const key in serverError) {
//             if (serverError[key]) {
//                 modelStateError += serverError[key] + '\n';
//             }
//         }
//     }

//     return Observable.throw(
//         modelStateError || 'Server Error'
//     );
// }
// }
