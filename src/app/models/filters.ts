export class Filters{
    ser:any
    au:any
    chNum:any
    status:any
    liked:boolean

    constructor(){
        this.ser = null
        this.au = null
        this.chNum = { from : null, to : null, custom : false}
        this.status = []
        this.liked = false
    }
}