export class Filters{
    ser:any
    au:any
    chNum:any
    status:any

    constructor(){
        this.ser = null
        this.au = null
        this.chNum = { from : null, to : null }
        this.status = null
    }
}