import {Injectable} from '@angular/core';
import {User} from '../_models/user';
import {Resolve, Router, ActivatedRouteSnapshot} from '@angular/router';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ListsResolver implements Resolve<User[]> {
    pageNumber = 1;
    pageSize = 5;
    likesParam = 'Likers';

    constructor(private userService: UserService, private router: Router,
        private alertify: AlertifyService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<User[]> {
        return this.userService.getUsers(this.pageNumber, this.pageSize, null, this.likesParam).pipe(
            catchError(error => {
                this.alertify.error('Problem retrieving data');
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }
}
// import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
// import { User } from '../_models/User';
// import { Injectable } from '@angular/core';
// import { UserService } from '../../../_services/user.service';
// import { AlertifyService } from '../../../_services/alertify.service';
// import { Observable } from 'rxjs/Observable';
// import 'rxjs/add/operator/catch';

// @Injectable()
// export class MemberListResolver implements Resolve<User[]> {
//     constructor(private userService: UserService,
//         private router: Router,
//         private alertify: AlertifyService) {

//         }

//         resolve(route: ActivatedRouteSnapshot): Observable<User[]> {
//             return this.userService.getUsers().catch(error => {
//                 this.alertify.error('Problem Retrieving Data');
//                 this.router.navigate(['/home']);
//                 return Observable.of(null);
//             });
//         }
// }
