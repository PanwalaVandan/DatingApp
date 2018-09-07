import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/user';

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

  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(this.baseUrl + 'users', httpOptions);
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
